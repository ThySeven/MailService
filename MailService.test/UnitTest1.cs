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

        var AutoMail = new AutoMail
        {
            SenderMail = "testmail1@gmail.com",
            ReceiverMail = "testmail2@gmail.com",
            Header = "testHeader",
            Content = "testcontent",
            DateTime = new DateTime(),
            Model = new InvoiceModel
            {
                PaidStatus = false,
                Price = 1000,
                Description = "testvase",
                Address = "testvej"
            }

        };

        // Setup mock behavior
        mockService.Setup(s => s.SendAsync(AutoMail));

        // Use the mock object in your test
        //var result = myClassUnderTest.MyMethod(mockService.Object);

        // Assert the result
        //Assert.True(result);
    }
}
