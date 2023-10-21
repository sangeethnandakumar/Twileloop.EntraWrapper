﻿<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/sangeethnandakumar/Twileloop.SST">
    <img src="https://1000logos.net/wp-content/uploads/2023/01/Microsoft-Azure-logo.png" alt="Logo" width="80">
  </a>

  <h1 align="center"> Twileloop.EntraID </h1>
  <small>Wrapper around Microsoft.Identity.Web</small>
  <h4 align="center"> PreBuilt Template | AzureAD B2C | Time Travel </h4>
</div>

## About
`Twileloop.EntraID` helps configuring AzureAD B2C by simplifying integration and giving a ready to use template. In the background this uses `Microsoft.Identity.Web`. 

<hr/>

## License
> Twileloop.EntraID - is licensed under the MIT License (Honouring Microsoft.Identity.Web). See the LICENSE file for more details.

#### This library is absolutely free. If it gives you a smile, A small coffee would be a great way to support my work. Thank you for considering it!
[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/sangeethnanda)

## Full Documentation
https://packages.twileloop.com/twileloop.entraid

## Usage

## Install Package

```powershell
dotnet add package Twileloop.EntraID
```

## Register In DI
Registration is very simple, Just paste this sniplet to `Program.cs` and configure according to your need
```csharp
builder.Services.AddEntraID(opt =>
{
    opt.EnableEventLogging = true;
    opt.GlobalAuthenticationFailureResponse = "You cannot consume the service.";
    opt.GlobalAuthorizationFailureResponse = "You dont' have enough privilages to access the requested endpoint.";
});
```

| Option     | Description | Screenshot  | Default Value 
| ---      | ---  | --- | ---
| EnableEventLogging   | Do you want to see security events from `Twileloop.EntraID`. This is usefull in knowing what is happening under the hood. Turn it on only during development or to troubleshoot  |    | false
| GlobalAuthenticationFailureResponse  | ✅
| GlobalAuthorizationFailureResponse   | ✅