using System;
using System.Collections.Generic;

namespace SpaFileReader
{
    public class SpaBuilder
    {
        private string _fileTitle;
        private DateTime _fileDateTime;
        private readonly List<Spectrum> _spectrums = new();

        public SpaBuilder FileTitle(string fileTitle)
        {
            _fileTitle = fileTitle;
            return this;
        }

        public SpaBuilder FileDateTime(DateTime fileDateTime)
        {
            _fileDateTime = fileDateTime;
            return this;
        }

        public SpaBuilder Spectrum(Spectrum spectrum)
        {
            _spectrums.Add(spectrum);
            return this;
        }

        public Spa Build()
        {
            return new()
            {
                FileDateTime = _fileDateTime,
                FileTitle = _fileTitle,
                Spectrums = _spectrums
            };
        }
    }
}
