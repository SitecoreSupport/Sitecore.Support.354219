namespace Sitecore.Support.Modules.EmailCampaign.Core.HtmlCorrection
{ 
    internal interface IHtmlCorrectionProcedure
    {
        CorrectionResult CorrectHtml(string inputHtml);
    }
}