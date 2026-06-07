namespace PracticalWork.Library.Contracts.v1.Books.Request
{
    public sealed record PostBookDetailsRequest
    {
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
    }
}