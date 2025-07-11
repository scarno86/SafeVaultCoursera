
using NUnit.Framework;

namespace SafeVault
{
    
    [TestFixture]
    public class TestInputValidation
    {
        [Test]
        public void TestForSQLInjection()
        {
            // Placeholder for SQL Injection test
        }
        [Test]
        public void TestForXSS()
        {
            string maliciousInput = "<script>alert('XSS');</script>";

            bool isValid = ValidationHelpers.IsValidXSSInput(maliciousInput);

            Console.WriteLine(isValid ? "XSS Test Failed" : "XSS Test Passed");
        }
    }
}
