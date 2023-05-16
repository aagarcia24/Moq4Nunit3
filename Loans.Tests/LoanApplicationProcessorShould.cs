using Loans.Domain.Applications;
using Moq;
using NUnit.Framework;

namespace Loans.Tests
{
    public class LoanApplicationProcessorShould
    {
        [Test]
        public void DeclineLowSalary()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000m);
            var application = new LoanApplication(42, product, amount, "Sarah", 25, "133 Pluralsight Drive, Draper, Utah", 64_999m);
            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            var mockCreditScorer = new Mock<ICreditScorer>();
            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.False);
        }

        delegate void ValidateCallback(string applicantName,
            int applicantAge,
            string applicantAddress,
            ref IdentityVerificationStatus status);

        [Test]
        public void Accept()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000m);
            var application = new LoanApplication(42, product, amount, "Sarah", 25, "133 Pluralsight Drive, Draper, Utah", 65_000m);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            mockIdentityVerifier.Setup(x => x.Validate("Sarah", 25, "133 Pluralsight Drive, Draper, Utah")).Returns(true);

            // Any Parameter
            //mockIdentityVerifier.Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            // out value
            //bool isValidOutValue = true;
            //mockIdentityVerifier.Setup(x => x.Validate("Sarah", 25, "133 Pluralsight Drive, Draper, Utah", out isValidOutValue));

            // ref value
            //mockIdentityVerifier.Setup(x => x.Validate("Sarah", 25, "133 Pluralsight Drive, Draper, Utah", ref It.Ref<IdentityVerificationStatus>.IsAny))
            //    .Callback(new ValidateCallback(
            //        (string applicantName, int applicantAge, string applicantAddress, ref IdentityVerificationStatus status)
            //    => status = new IdentityVerificationStatus(true)));

            var mockCreditScorer = new Mock<ICreditScorer>();

            // Property Hierarchy
            //var mockScoreValue = new Mock<ScoreValue>();
            //mockScoreValue.Setup(x => x.Score).Returns(300);
            //var mockScoreResult = new Mock<ScoreResult>();
            //mockScoreResult.Setup(x => x.ScoreValue).Returns(mockScoreValue.Object);
            //mockCreditScorer.Setup(x => x.ScoreResult).Returns(mockScoreResult.Object);

            // Individual property
            //mockCreditScorer.Setup(x => x.Score).Returns(300);

            mockCreditScorer.SetupAllProperties();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);
            mockCreditScorer.SetupProperty(x => x.Count);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.True);
            Assert.That(mockCreditScorer.Object.Count, Is.EqualTo(1));
        }

        [Test]
        public void NullReturnExample()
        {
            var mock = new Mock<INullExample>();
            mock.Setup(x => x.SomeMethod());
            //.Returns<string>(null);

            string mockReturnValue = mock.Object.SomeMethod();

            Assert.That(mockReturnValue, Is.Null);
        }
    }

    public interface INullExample
    {
        string SomeMethod();
    }
}