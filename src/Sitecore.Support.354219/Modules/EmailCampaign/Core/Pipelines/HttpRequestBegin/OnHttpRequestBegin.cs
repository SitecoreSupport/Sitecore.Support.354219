namespace Sitecore.Support.Modules.EmailCampaign.Core.Pipelines.HttpRequestBegin
{
    using Sitecore.EmailCampaign.Model.Web.Settings;
    using Sitecore.ExM.Framework.Diagnostics;
    using Sitecore.Modules.EmailCampaign.Core;
    using Sitecore.Modules.EmailCampaign.Core.Contacts;
    using Sitecore.Modules.EmailCampaign.Core.Personalization;
    using Sitecore.Modules.EmailCampaign.Services;
    using Sitecore.Pipelines.HttpRequest;
    using Sitecore.Support.Modules.EmailCampaign.Core.HtmlCorrection;

    public class OnHttpRequestBegin : Sitecore.Modules.EmailCampaign.Core.Pipelines.HttpRequestBegin.OnHttpRequestBegin
    {
        private PipelineHelper _pipelineHelper;
        private ILogger _logger;
        public OnHttpRequestBegin(IContactService contactService, ILogger logger, IExmCampaignService exmCampaignService, PipelineHelper pipelineHelper) : base(contactService, logger, exmCampaignService, pipelineHelper)
        {
            _pipelineHelper = pipelineHelper;
            _logger = logger;
        }

        protected override void ApplyOutputFilter(HttpRequestArgs args)
        {
            if (((args.HttpContext.Request.HttpMethod != "POST") && (args.HttpContext.Response.ContentType == "text/html")) && args.Url.QueryString.Contains(GlobalSettings.OnlineVersionQueryStringKey))
            {
                PersonalizationManager personalizationManager = this.GetPersonalizationManager();
                if ((personalizationManager != null) && (ExmContext.Message != null))
                {
                    StreamHtmlCorrector corrector = new StreamHtmlCorrector(args.Context.Response.Filter, args.Context.Response.ContentEncoding);
                    corrector.AddHtmlCorrector(new TokenHtmlCorrectionProcedure(personalizationManager));
                    corrector.AddHtmlCorrector(new LinkHtmlCorrectionProcedure(this._pipelineHelper, this._logger, ExmContext.Message, ExmContext.ContactIdentifier));
                    args.HttpContext.Response.Filter = corrector;
                }
            }
        }

    }
}