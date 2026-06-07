using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Controllers.Mappers.v1;

namespace PracticalWork.Library.Controllers.Api.v1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/library")]
public sealed class LibraryController: Controller
{
    private readonly ILibraryService _libraryService;

    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }
    
    /// <summary>
    /// Выдача книги
    /// </summary>
    [HttpPost]
    [Route("/borrow/{bookId:guid}/{readerId:guid}")]
    [Produces("application/json")]
    [ProducesResponseType( 204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> BorrowBook(Guid bookId, Guid readerId)
    {
        await _libraryService.BorrowBook(bookId, readerId);
        return Created();
    }
    
    /// <summary>
    /// Возврат книги
    /// </summary>
    [HttpPost]
    [Route("/return/{bookId:guid}/{readerId:guid}")]
    [Produces("application/json")]
    [ProducesResponseType( 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ReturnBook(Guid bookId, Guid readerId)
    {
        await _libraryService.ReturnBook(bookId, readerId);
        return Ok();
    }
    
    /// <summary>
    /// Получение деталей книги
    /// </summary>
    [HttpGet]
    [Route("/books/{idOrTitle}/details")]
    [Produces("application/json")]
    [ProducesResponseType<BookDetailsResponse>( 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetBookDetails(string idOrTitle)
    {
        var result = await _libraryService.GetBookDetails(idOrTitle.ToBookDetailsQuery());
        return Ok(result.book.ToBookDetailsResponse(result.bookId));
    }
    
    /// <summary>
    /// Получение не архивированных книг постранично
    /// </summary>
    [HttpPost]
    [Route("/books")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetBooksPage(PaginationRequest request)
    {
        var result = await _libraryService
            .GetNonArchivedBooksPage(request.ToPageRequest());
        return Ok(result.ToBookWithIssuanceCursorPaginationResponse());
    }
}
