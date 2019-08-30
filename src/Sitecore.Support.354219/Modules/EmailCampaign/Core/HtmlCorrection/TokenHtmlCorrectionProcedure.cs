namespace Sitecore.Support.Modules.EmailCampaign.Core.HtmlCorrection
{
    using Sitecore.Modules.EmailCampaign.Core.Personalization;

    internal class TokenHtmlCorrectionProcedure : IHtmlCorrectionProcedure
    {
        private readonly PersonalizationManager personalizationManager;

        internal TokenHtmlCorrectionProcedure(PersonalizationManager manager)
        {
            this.personalizationManager = manager;
        }

        CorrectionResult IHtmlCorrectionProcedure.CorrectHtml(string inputHtml)
        {
            if (this.personalizationManager != null)
            {
                var value = this.personalizationManager.ModifyText(inputHtml);
                if (!string.IsNullOrEmpty(value))
                {
                    return new CorrectionResult(CorrectionResultState.Success, value);
                }
            }

            return new CorrectionResult(CorrectionResultState.Failed, string.Empty);
        }
    }
}