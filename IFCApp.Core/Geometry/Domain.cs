using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Geometry
{
    public struct Domain
    {
        public double Start { get; set; }
        public double End { get; set; }
        public double Length { get => End > Start ? End - Start : Start - End; }
        public double Mid { get => (Start+End)/2; }

        public Domain() { }
        public Domain(double start, double end)
        {
            Start = start;
            End = end;
        }

        public (Domain, Domain) Split(BBox box)
        {
            var d1Start = this.Start < this.End ? this.Start : this.End;
            var d1End = box.Min.X;
            var d2Start = box.Max.X;
            var d2End = this.End > this.Start ? this.End : this.Start;
            return (new Domain(d1Start, d1End), new Domain(d2Start, d2End));
        }

    }
}
