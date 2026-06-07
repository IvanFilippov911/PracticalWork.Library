namespace PracticalWork.Library.Exceptions
{
	/// <summary>
	/// Исключение для обозначения, что какие-то данные не найдены
	/// </summary>
	public class NotFoundException : AppException
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		public NotFoundException()
		{
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="message">Сообщение об ошибке</param>
		public NotFoundException(string message)
			: base(message)
		{
		}
	}
}
