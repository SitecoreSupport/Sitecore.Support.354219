namespace Sitecore.Support.Modules.EmailCampaign.Core.HtmlCorrection
{
    internal struct CorrectionResult
    {
        internal CorrectionResult(CorrectionResultState state, string html)
            : this()
        {
            this.State = state;
            this.CorrectedHtml = html;
        }

        internal string CorrectedHtml { get; private set; }

        internal CorrectionResultState State { get; private set; }
    }
}