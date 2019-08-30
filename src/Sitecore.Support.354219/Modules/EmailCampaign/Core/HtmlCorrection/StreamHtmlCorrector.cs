namespace Sitecore.Support.Modules.EmailCampaign.Core.HtmlCorrection
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Sitecore.Diagnostics;

    internal class StreamHtmlCorrector : Stream
    {
        private readonly Stream baseStream;
        private readonly Encoding contentEncoding;
        private readonly List<IHtmlCorrectionProcedure> correctors = new List<IHtmlCorrectionProcedure>();

        internal StreamHtmlCorrector(Stream responseStream, Encoding contentEncoding)
        {
            Assert.ArgumentNotNull(responseStream, "responseStream");
            Assert.ArgumentNotNull(contentEncoding, "contentEncoding");

            this.baseStream = responseStream;
            this.contentEncoding = contentEncoding;
        }

        public override bool CanRead
        {
            get { return this.baseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.baseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this.baseStream.CanWrite; }
        }

        public override long Length
        {
            get { return this.baseStream.Length; }
        }

        public override long Position
        {
            get
            {
                return this.baseStream.Position;
            }
            set
            {
                this.baseStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.baseStream.SetLength(value);
        }

        public override void Flush()
        {
            this.baseStream.Flush();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var originalText = this.contentEncoding.GetString(buffer, offset, count);

            var modifiedHtml = originalText;

            foreach (var corrector in this.correctors)
            {
                var result = corrector.CorrectHtml(modifiedHtml);

                if (result.State == CorrectionResultState.Success)
                {
                    modifiedHtml = result.CorrectedHtml;
                }
            }

            buffer = this.contentEncoding.GetBytes(modifiedHtml);
            this.baseStream.Write(buffer, 0, buffer.Length);
        }

        internal void AddHtmlCorrector(IHtmlCorrectionProcedure correctionProcedure)
        {
            if (!this.correctors.Contains(correctionProcedure))
            {
                this.correctors.Add(correctionProcedure);
            }
        }
    }
}