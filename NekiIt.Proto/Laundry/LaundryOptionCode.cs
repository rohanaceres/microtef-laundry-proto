namespace NekiIt.Proto.Laundry
{
    /// <summary>
    /// Option of laundry service.
    /// </summary>
    internal enum LaundryOptionCode
    {
        /// <summary>
        /// Invalid laundry service.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Wash clothes.
        /// </summary>
        Wash = 1,
        /// <summary>
        /// Dry clothes.
        /// </summary>
        Dry = 2
    }

    /// <summary>
    /// Responsible for provide <see cref="LaundryOptionCode"/> useful extensions.
    /// </summary>
    internal static class LaundryOptionCodeExtension
    {
        /// <summary>
        /// Responsible for transforming a string into the corresponding 
        /// <see cref="LaundryOptionCode"/>.
        /// </summary>
        /// <param name="self">The string to be transformed.</param>
        /// <returns>The corresponding code, or <see cref="LaundryOptionCode.Undefined"/> 
        /// if the string were not recognized.</returns>
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
