using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Tests.Common;

namespace PracticalWork.Library.Tests.Models;

public sealed class BookBorrowTests
{
    [Fact]
    public void CreateBookBorrow_SetsIssuedStatusAndDueDate()
    {
        var timeProvider = new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc));

        var borrow = BookBorrow.CreateBookBorrow(timeProvider);

        Assert.Equal(BookIssueStatus.Issued, borrow.Status);
        Assert.Equal(new DateOnly(2026, 5, 26), borrow.BorrowDate);
        Assert.Equal(new DateOnly(2026, 6, 25), borrow.DueDate);
    }

    [Fact]
    public void ReturnBookBorrow_WhenReturnedOnTime_SetsReturnedStatus()
    {
        var timeProvider = new TestTimeProvider(new DateTime(2026, 5, 20, 8, 0, 0, DateTimeKind.Utc));
        var book = new Book { Status = BookStatus.Borrow };
        var borrow = new BookBorrow
        {
            Book = book,
            DueDate = new DateOnly(2026, 5, 25),
            Status = BookIssueStatus.Issued
        };

        borrow.ReturnBookBorrow(timeProvider);

        Assert.Equal(BookIssueStatus.Returned, borrow.Status);
        Assert.Equal(new DateOnly(2026, 5, 20), borrow.ReturnDate);
        Assert.Equal(BookStatus.Available, book.Status);
    }

    [Fact]
    public void ReturnBookBorrow_WhenReturnedAfterDueDate_SetsOverdueStatus()
    {
        var timeProvider = new TestTimeProvider(new DateTime(2026, 5, 30, 8, 0, 0, DateTimeKind.Utc));
        var book = new Book { Status = BookStatus.Borrow };
        var borrow = new BookBorrow
        {
            Book = book,
            DueDate = new DateOnly(2026, 5, 25),
            Status = BookIssueStatus.Issued
        };

        borrow.ReturnBookBorrow(timeProvider);

        Assert.Equal(BookIssueStatus.Overdue, borrow.Status);
        Assert.Equal(new DateOnly(2026, 5, 30), borrow.ReturnDate);
        Assert.Equal(BookStatus.Available, book.Status);
    }
}
