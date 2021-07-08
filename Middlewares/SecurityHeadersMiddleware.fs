namespace TodoApi.Middlewares

open Microsoft.Extensions.Primitives
open Microsoft.AspNetCore.Http

type SecurityHeadersMiddleware private () =

    member val _next : RequestDelegate = null with get, set

    new (next : RequestDelegate) as this = SecurityHeadersMiddleware() then
        this._next <- next

    member this.InvokeAsync (context : HttpContext) =
        context.Response.Headers.Add("referrer-policy", StringValues("strict-origin-when-cross-origin"))
        context.Response.Headers.Add("x-content-type-options", StringValues("nosniff"))
        context.Response.Headers.Add("x-frame-options", StringValues("DENY"))
        context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", StringValues("none"))
        context.Response.Headers.Add("x-xss-protection", StringValues("1; mode=block"))
        context.Response.Headers.Add("Expect-CT", StringValues("max-age=0, enforce, report-uri=\"https://example.report-uri.com/r/d/ct/enforce\""))
        context.Response.Headers.Add("Feature-Policy", StringValues(
                                        "accelerometer 'none';" +
                                        "ambient-light-sensor 'none';" +
                                        "autoplay 'none';" +
                                        "battery 'none';" +
                                        "camera 'none';" +
                                        "display-capture 'none';" +
                                        "document-domain 'none';" +
                                        "encrypted-media 'none';" +
                                        "execution-while-not-rendered 'none';" +
                                        "execution-while-out-of-viewport 'none';" +
                                        "gyroscope 'none';" +
                                        "magnetometer 'none';" +
                                        "microphone 'none';" +
                                        "midi 'none';" +
                                        "navigation-override 'none';" +
                                        "payment 'none';" +
                                        "picture-in-picture 'none';" +
                                        "publickey-credentials-get 'none';" +
                                        "sync-xhr 'none';" +
                                        "usb 'none';" +
                                        "wake-lock 'none';" +
                                        "xr-spatial-tracking 'none';"
                                    ))

        context.Response.Headers.Add("Content-Security-Policy", StringValues(
                                        "base-uri 'none';" +
                                        "block-all-mixed-content;" +
                                        "child-src 'none';" +
                                        "connect-src 'none';" +
                                        "default-src 'none';" +
                                        "font-src 'none';" +
                                        "form-action 'none';" +
                                        "frame-ancestors 'none';" +
                                        "frame-src 'none';" +
                                        "img-src 'none';" +
                                        "manifest-src 'none';" +
                                        "media-src 'none';" +
                                        "object-src 'none';" +
                                        "sandbox;" +
                                        "script-src 'none';" +
                                        "script-src-attr 'none';" +
                                        "script-src-elem 'none';" +
                                        "style-src 'none';" +
                                        "style-src-attr 'none';" +
                                        "style-src-elem 'none';" +
                                        "upgrade-insecure-requests;" +
                                        "worker-src 'none';"
                                    ))

        let task = this._next.Invoke(context)
        task