using System.Net;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using WebApi.Extractors;

namespace WebApiUnitTests;

public class IpAddressExtractorTests
{
    [Fact]
    public void ExtractIpAddress_WithRemoteIpAddress_ReturnsRemoteIpAddress()
    {
        // Arrange
        var remoteIpAddress = IPAddress.Parse("192.168.1.1");
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = remoteIpAddress;

        // Act
        var result = context.ExtractIpAddress();

        // Assert
        Assert.Equal(remoteIpAddress, result);
    }

    [Theory]
    [MemberData(nameof(ExtractedIpTestData))]
    public void ExtractIpAddress_WithHeaders_ReturnsCorrectIpAddress(string headerName, string headerValue,
        IPAddress expectedIpAddress)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers[headerName] = headerValue;

        // Act
        var result = context.ExtractIpAddress();

        // Assert
        Assert.Equal(expectedIpAddress, result);
    }

    public static IEnumerable<object[]> ExtractedIpTestData =>
        new List<object[]>
        {
            // Test data for X-Forwarded-For header
            new object[] { "X-Forwarded-For", "192.168.1.2", IPAddress.Parse("192.168.1.2") },
            new object[] { "X-Forwarded-For", "192.168.1.3, 10.0.0.1", IPAddress.Parse("192.168.1.3") },
            new object[]
            {
                "X-Forwarded-For", "2001:0db8:85a3:0000:0000:8a2e:0370:7334",
                IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334")
            },

            // Test data for X-Real-IP header
            new object[] { "X-Real-IP", "172.16.0.1", IPAddress.Parse("172.16.0.1") },
            new object[]
            {
                "X-Real-IP", "2001:0db8:85a3:0000:0000:8a2e:0370:7335",
                IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7335")
            }
        };

    [Fact]
    public void ExtractIpAddress_WithNoIpAddress_ReturnsDomainException()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act & Assert
        Assert.Throws<DomainException>(() => context.ExtractIpAddress());
    }
}