namespace LocalUtils
{
    public static class MathUtils
    {
        public static float Remap(float value, float min, float max, float toMin, float toMax)
        {
            return (value - min) / (max - min) * (toMax - toMin) + toMin;
        }
    }
}