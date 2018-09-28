namespace Nop.Plugin.Payments.PayPalPlusBrasil
{
    /// <summary>
    /// Represents payment processor transaction mode
    /// </summary>
    public enum TransactMode
    {
        /// <summary>
        /// Authorize
        /// </summary>
        Authorize = 0,
        /// <summary>
        /// Authorize and capture
        /// </summary>
        AuthorizeAndCapture= 2
    }
}
