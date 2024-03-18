﻿using KiwiLoad.Core.ValueObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace KiwiLoad.Core.Authentication;
public class TokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string Token = "BAt4IElJoNDnvinqH6gMBhBO9Y8YLLBk0N4SZkijQEz9VBqVfuXuUSzFDDClD3Ya";
    private readonly IMemoryCache memoryCache;

    public TokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IMemoryCache memoryCache
        ) :
        base(options, logger, encoder, clock)
    {
        this.memoryCache=memoryCache;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        await Task.Yield();
        var token = Request.Headers["Authorization"];
        token = (string)token.ToString().Replace("Bearer ", "");

        // YDYk2xyRSHy2xYDOMEQyDg77YZWSX0B427bO4OC2ayHSNZ9iYVMRC7UMK1Rx4p3C
        if (memoryCache.TryGetValue<string>(token, out var username))
        {
            username = username ?? throw new ArgumentException("Username not found");
            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, username),
            };

            var identity = new System.Security.Claims.ClaimsIdentity(claims, Scheme.Name);
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        else
        {
            return AuthenticateResult.Fail("Unauthorized");
        }
    }
}
