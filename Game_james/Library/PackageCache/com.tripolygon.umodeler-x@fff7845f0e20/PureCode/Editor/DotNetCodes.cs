#if NET_STANDARD_2_1

namespace Tripolygon.UModelerX.Runtime.Dotnets
{
    [UnityEditor.InitializeOnLoad]
    public class DotnetCodes : IArray
    {
        public static Array Current { get; set; }
        static Array()
        {
            Current = new Array();
        }

        public Array()
        {
            UMXArray.array = this;
        }
        void IArray.Fill<T>(T[] array, T value)
        {
            System.Array.Fill<T>(array, value);
        }
        void IArray.Fill<T>(T[] array, T value, int startIndex, int count)
        {
            System.Array.Fill<T>(array, value, startIndex, count);
        }
    }
}
#endif
