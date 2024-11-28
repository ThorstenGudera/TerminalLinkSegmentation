namespace AvoidAGrabCutEasy
{
    internal class LumMapApplicationSettings
    {
        public float Factor1 { get; internal set; } = 1.0f;
        public double Threshold { get; internal set; } = 0.5;
        public float Factor2 { get; internal set; } = 1.0f;
        public double Exponent1 { get; internal set; } = 1.0;
        public double Exponent2 { get; internal set; } = 1.0;
        public double ThMultiplier { get; internal set; } = Math.Pow(10.0, -10.0);
        public bool MultAuto { get; internal set; } = true;
        public bool ValsLessThanTh { get; internal set; } = true;
        public bool DoFirstMultiplication { get; internal set; } = true;
        public bool DoSecondMultiplication { get; internal set; } = true;
    }
}