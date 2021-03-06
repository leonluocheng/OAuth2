using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OAuth2.Client;
using OAuth2.Client.Impl;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Tests.Client.Impl
{
    [TestFixture]
    public class GoogleClientTests
    {
        private const string Content = "{\"email\":\"email\",\"given_name\":\"name\",\"family_name\":\"surname\",\"id\":\"id\"}";
        private const string ContentWithPicture = "{\"email\":\"email\",\"given_name\":\"name\",\"family_name\":\"surname\",\"id\":\"id\",\"picture\":\"picture\"}";

        private GoogleClientDescendant _descendant;

        [SetUp]
        public void SetUp()
        {
            _descendant = new GoogleClientDescendant(Substitute.For<IRequestFactory>(), Substitute.For<IClientConfiguration>());
        }

        [Test]
        public void Should_ReturnCorrectAccessCodeServiceEndpoint()
        {
            // act
            var endpoint = _descendant.GetAccessCodeServiceEndpoint();

            // assert
            endpoint.BaseUri.Should().Be("https://accounts.google.com");
            endpoint.Resource.Should().Be("/o/oauth2/auth");
        }

        [Test]
        public void Should_ReturnCorrectAccessTokenServiceEndpoint()
        {
            // act
            var endpoint = _descendant.GetAccessTokenServiceEndpoint();

            // assert
            endpoint.BaseUri.Should().Be("https://accounts.google.com");
            endpoint.Resource.Should().Be("/o/oauth2/token");
        }

        [Test]
        public void Should_ReturnCorrectUserInfoServiceEndpoint()
        {
            // act
            var endpoint = _descendant.GetUserInfoServiceEndpoint();

            // assert
            endpoint.BaseUri.Should().Be("https://www.googleapis.com");
            endpoint.Resource.Should().Be("/oauth2/v1/userinfo");
        }

        [Test]
        public void ShouldNot_Throw_WhenParsingUserInfoAndPictureIsNotAvailable()
        {
            // act & assert
            _descendant.Invoking(x => x.ParseUserInfo(Content)).Should().NotThrow();
        }

        [Test]
        public void Should_ParseAllFieldsOfUserInfo_WhenCorrectContentIsPassed()
        {
            // act
            var info = _descendant.ParseUserInfo(ContentWithPicture);

            //  assert
            info.Id.Should().Be("id");
            info.FirstName.Should().Be("name");
            info.LastName.Should().Be("surname");
            info.Email.Should().Be("email");
            info.PhotoUri.Should().Be("picture");
        }

        class GoogleClientDescendant : GoogleClient
        {
            public GoogleClientDescendant(IRequestFactory factory, IClientConfiguration configuration)
                : base(factory, configuration)
            {
            }

            public Endpoint GetAccessCodeServiceEndpoint()
            {
                return AccessCodeServiceEndpoint;
            }

            public Endpoint GetAccessTokenServiceEndpoint()
            {
                return AccessTokenServiceEndpoint;
            }

            public Endpoint GetUserInfoServiceEndpoint()
            {
                return UserInfoServiceEndpoint;
            }

            public new UserInfo ParseUserInfo(string content)
            {
                return base.ParseUserInfo(content);
            }
        } 
    }
}