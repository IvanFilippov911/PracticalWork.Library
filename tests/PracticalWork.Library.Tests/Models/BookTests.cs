using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.BookModels;

namespace PracticalWork.Library.Tests.Models;

public sealed class BookTests
{
    [Fact]
    public void Archive_WhenBookIsAvailable_SetsArchivedFlags()
    {
        var book = new Book
        {
            Title = "Clean Architecture",
            Authors = ["Robert C. Martin"],
            Description = "desc",
            Year = 2018,
            Category = BookCategory.FictionBook,
            Status = BookStatus.Available,
            IsArchived = false
        };

        book.Archive();

        Assert.True(book.IsArchived);
        Assert.Equal(BookStatus.Archived, book.Status);
    }

    [Fact]
    public void Archive_WhenBookIsBorrowed_ThrowsInvalidOperationException()
    {
        var book = new Book
        {
            Title = "Domain-Driven Design",
            Authors = ["Eric Evans"],
            Description = "desc",
            Year = 2003,
            Category = BookCategory.FictionBook,
            Status = BookStatus.Borrow
        };

        var action = () => book.Archive();

        Assert.Throws<InvalidOperationException>(action);
    }
}
