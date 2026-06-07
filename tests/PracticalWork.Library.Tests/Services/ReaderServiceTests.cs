using Moq;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.ReaderModels;
using PracticalWork.Library.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Library.Tests.Services;

public sealed class ReaderServiceTests
{
    [Fact]
    public async Task CreateReader_WhenPhoneAlreadyExists_ThrowsException()
    {
        var repository = new Mock<IReaderRepository>();
        repository.Setup(x => x.IsExistReader("79990000000")).ReturnsAsync(true);
        var service = new ReaderService(repository.Object, new Mock<IMessageProducer>().Object, new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var exception = await Assert.ThrowsAsync<ReaderServiceException>(() => service.CreateReader(new Reader
        {
            PhoneNumber = "79990000000",
            FullName = "Reader",
            ExpiryDate = new DateOnly(2026, 12, 31)
        }));

        Assert.Equal("Phone number is not unique", exception.Message);
    }

    [Fact]
    public async Task CreateReader_WhenSuccessful_ActivatesReaderAndPublishesEvent()
    {
        var repository = new Mock<IReaderRepository>();
        var producer = new Mock<IMessageProducer>();
        repository.Setup(x => x.IsExistReader(It.IsAny<string>())).ReturnsAsync(false);
        repository.Setup(x => x.CreateReader(It.IsAny<Reader>())).ReturnsAsync(Guid.NewGuid());
        var reader = new Reader
        {
            FullName = "Reader Name",
            PhoneNumber = "79990000000",
            Email = "reader@test.local",
            ExpiryDate = new DateOnly(2026, 12, 31)
        };
        var service = new ReaderService(repository.Object, producer.Object, new TestTimeProvider(new DateTime(2026, 5, 26, 11, 0, 0, DateTimeKind.Utc)));

        await service.CreateReader(reader);

        Assert.True(reader.IsActive);
        repository.Verify(x => x.CreateReader(reader), Times.Once);
        producer.Verify(x => x.ProduceReaderCreateAsync(It.IsAny<Contracts.Events.ReaderCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task ExtendExpiryDate_WhenNewDateIsNotFuture_ThrowsException()
    {
        var repository = new Mock<IReaderRepository>();
        repository.Setup(x => x.GetReader(It.IsAny<Guid>())).ReturnsAsync(new Reader
        {
            IsActive = true,
            ExpiryDate = new DateOnly(2026, 6, 1)
        });
        var service = new ReaderService(repository.Object, new Mock<IMessageProducer>().Object, new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var exception = await Assert.ThrowsAsync<ReaderServiceException>(() =>
            service.ExtendExpiryDate(Guid.NewGuid(), new DateOnly(2026, 6, 1)));

        Assert.Equal("Необходимо продлить карточку на будущую дату", exception.Message);
        repository.Verify(x => x.UpdateReader(It.IsAny<Guid>(), It.IsAny<Reader>()), Times.Never);
    }

    [Fact]
    public async Task CloseReader_WhenBorrowedBooksExist_ReturnsFlagAndDoesNotDeactivateReader()
    {
        var repository = new Mock<IReaderRepository>();
        var reader = new Reader
        {
            IsActive = true,
            BorrowBooks =
            [
                new Book { Title = "Borrowed", Authors = ["A"], Description = "D" }
            ]
        };
        repository.Setup(x => x.GetReaderWithBorrowBooks(It.IsAny<Guid>())).ReturnsAsync(reader);
        var service = new ReaderService(repository.Object, new Mock<IMessageProducer>().Object, new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var result = await service.CloseReader(Guid.NewGuid());

        Assert.True(result.borrowBooksExist);
        Assert.Single(result.borrowBooks);
        repository.Verify(x => x.UpdateReader(It.IsAny<Guid>(), It.IsAny<Reader>()), Times.Never);
    }

    [Fact]
    public async Task CloseReader_WhenNoBorrowedBooks_DeactivatesReaderAndPublishesEvent()
    {
        var repository = new Mock<IReaderRepository>();
        var producer = new Mock<IMessageProducer>();
        var reader = new Reader
        {
            FullName = "Reader",
            IsActive = true,
            BorrowBooks = []
        };
        repository.Setup(x => x.GetReaderWithBorrowBooks(It.IsAny<Guid>())).ReturnsAsync(reader);
        var service = new ReaderService(repository.Object, producer.Object, new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var result = await service.CloseReader(Guid.NewGuid());

        Assert.False(result.borrowBooksExist);
        Assert.False(reader.IsActive);
        Assert.Equal(new DateOnly(2026, 5, 26), reader.ExpiryDate);
        repository.Verify(x => x.UpdateReader(It.IsAny<Guid>(), reader), Times.Once);
        producer.Verify(x => x.ProduceReaderCloseAsync(It.IsAny<Contracts.Events.ReaderClosedEvent>()), Times.Once);
    }
}
