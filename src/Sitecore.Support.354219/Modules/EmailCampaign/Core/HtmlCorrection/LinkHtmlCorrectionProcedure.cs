namespace Sitecore.Support.Modules.EmailCampaign.Core.HtmlCorrection
{
    using System;
    using Sitecore.Diagnostics;
    using Sitecore.ExM.Framework.Diagnostics;
    using Sitecore.Modules.EmailCampaign.Core;
    using Sitecore.Support.Modules.EmailCampaign.Core.Links;
    using Sitecore.Modules.EmailCampaign.Core.Pipelines.GenerateLink;
    using Sitecore.Modules.EmailCampaign.Messages;
    using Sitecore.XConnect;
    public class LinkHtmlCorrectionProcedure : IHtmlCorrectionProcedure
    {
        private readonly ILogger _logger;
        private readonly PipelineHelper _pipelineHelper;
        private readonly MessageItem _message;
        private readonly ContactIdentifier _contactIdentifier;

        internal LinkHtmlCorrectionProcedure(PipelineHelper pipelineHelper, ILogger logger, MessageItem message, ContactIdentifier contactIdentifier)
        {
            Assert.ArgumentNotNull(pipelineHelper, "pipelineHelper");
            Assert.ArgumentNotNull(logger, "logger");
            Assert.ArgumentNotNull(message, nameof(message));
            Assert.ArgumentNotNull(contactIdentifier, nameof(contactIdentifier));

            _pipelineHelper = pipelineHelper;
            _logger = logger;
            _message = message;
            _contactIdentifier = contactIdentifier;
        }

        CorrectionResult IHtmlCorrectionProcedure.CorrectHtml(string html)
        {
            try
            {
                Assert.ArgumentNotNull(html, "html");

                var linksManager = new LinksManager(html, Sitecore.Modules.EmailCampaign.Core.Links.LinkTypes.Href);
                var mailMessage = _message as MailMessageItem;
                if (mailMessage == null)
                {
                    return new CorrectionResult(CorrectionResultState.Failed, html);
                }

                mailMessage.ContactIdentifier = _contactIdentifier;

                var newHtml = linksManager.Replace(link =>
                {
                    var args = new GenerateLinkPipelineArgs(link, mailMessage, false);
                    _pipelineHelper.RunPipeline(Sitecore.EmailCampaign.Model.Constants.ModifyHyperlinkPipeline, args);
                    return args.Aborted ? null : args.GeneratedUrl;
                });

                return new CorrectionResult(CorrectionResultState.Success, newHtml);
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return new CorrectionResult(CorrectionResultState.Failed, html);
            }
        }
    }
}
