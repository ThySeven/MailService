using Moq;

using MailService.Services;
using MailService.Models;
// Your test class
public class MyTestClass
{
    public void MyTestMethod()
    {
        // Create a mock object for an interface
        var mockService = new Mock<IDeliveryService>();

        var mail = new MailModel
        {
            ReceiverMail = "testmail2@gmail.com",
            Header = "testHeader",
            Content = "testcontent",
        };

        // Setup mock behavior
        mockService.Setup(s => s.SendAsync(mail));

        // Use the mock object in your test
        //var result = myClassUnderTest.MyMethod(mockService.Object);

        // Assert the result
        //Assert.True(result);
    }
}
