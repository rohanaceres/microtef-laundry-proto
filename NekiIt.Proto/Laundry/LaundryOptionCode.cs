namespace NekiIt.Proto.Laundry
{
    // TODO: Doc.
    internal enum LaundryOptionCode
    {
        Undefined = 0,
        Wash = 1,
        Dry = 2
    }

    // TODO: Doc.
    internal static class LaundryOptionCodeExtension
    {
        public static LaundryOptionCode ToLaundryOptionCode (this string self)
        {
            switch (self)
            {
                case "LAVAGEL":
                    return LaundryOptionCode.Wash;
                case "SECAGEM":
                    return LaundryOptionCode.Dry;
                default:
                    return LaundryOptionCode.Undefined;
            }
        }
    }
}
