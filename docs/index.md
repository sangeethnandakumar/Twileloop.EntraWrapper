---
layout: default
---

## About
`Twileloop.EntraWrapper` helps configuring AzureAD B2C by simplifying integration and giving a ready to use template. In the background this uses `Microsoft.Identity.Web`. 

<hr/>

## License
> Twileloop.EntraWrapper - is licensed under the MIT License (Honouring Microsoft.Identity.Web). See the LICENSE file for more details.

#### This library is absolutely free. If it gives you a smile, A small coffee would be a great way to support my work. Thank you for considering it!
[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/sangeethnanda)

## Full Documentation
https://packages.twileloop.com/Twileloop.EntraWrapper

## Usage

## Install Package

```powershell
dotnet add package Twileloop.EntraWrapper
```

## Register In DI
Registration is very simple, Just paste this sniplet to `Program.cs` and simply configure by looking the below table
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
| EnableEventLogging   | Do you want to see security events from `Twileloop.EntraWrapper`? This is a very useful feature. Simply turning it `true` is not enough. You need to tell `Twileloop.EntraWrapper` where you want to write event logs because you may want it to be in the console, files, or any custom implementation you have. We will discuss that in the next section. Turn it on only during development or troubleshooting since logging security events is not recommended for production scenarios. *Refer screenshots |  ![image](https://github.com/sangeethnandakumar/Twileloop.EntraWrapper/assets/24974154/75371366-eff5-4334-b9b5-0a610e591e0d) | false
| GlobalAuthenticationFailureResponse | What you want to show during an Authentication failure to the user. If preferred, Specify the text you want to show along 401 UNAUTHORIZED. *Refer screenshots |  ![image](https://github.com/sangeethnandakumar/Twileloop.EntraWrapper/assets/24974154/b8e001f5-e741-40a6-a196-d7c29bf9e964) | Empty
| GlobalAuthorizationFailureResponse  | What you want to show during an Authorization failure to the user. If preferred, Specify the text you want to show along 403 FORBIDDEN. For Authorization scenarios, if you prefer you can override this global message also. We will discuss below on that. *Refer screenshots  | ![image](https://github.com/sangeethnandakumar/Twileloop.EntraWrapper/assets/24974154/e570582c-77cd-425c-8dc8-b2b1dc7cab2e) | Empty

## Register the middlewares
After `AddEntraID` is called in `Program.cs` to register necessary dependencies, Register the `Authentication` and `Authorization` middlewares

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

## That's It. Now just know the "3 Main Interfaces"
To simplify and give you the maximum customization possibilities, I created 3 interfaces that support in 3 major tasks

- IEntraEventLogger 
- IEntraConfigurationResolver
- IEntraAuthorizationResolver

Here is what each interface do

| Interface     | Description | Example Scenerio
| ---      | ---  | ---
| IEntraEventLogger   | Write security logs to wherever you prefer | `Twileloop.EntraWrapper` delivers security event logs up to this interface. From here you can channel it to anywhere you need with your custom logic. Eg: Console, File, Database, Serilog, Seq, etc..
| IEntraConfigurationResolver   | Allows you to pick AzureAD configuration from anywhere | `Twileloop.EntraWrapper` gives a trigger to this interface when it needs configuration to set up API security. You can write your custom logic to read configuration from anywhere you like including config files like appsettings.json, databases, API responses, etc.. Then put configuration information into an `EntraConfig` record instance and return back to `Twileloop.EntraWrapper`
| IEntraAuthorizationResolver   | Allows you to define which request to pass and which to block | `Twileloop.EntraWrapper` gives a hit to this interface with enough information and executes your custom code to perform authorization. You can write custom code that checks for roles, scopes etc.. `Twileloop.EntraWrapper` will deliver parsed JWT token, current running policy against [Authorize], HttpRequest etc.. so you can make the decision and inform back/return with a boolean indicating allow or block.

> Hope the above is clear.

# Let's create 3 classes to implement these 3 interfaces
Create concrete classes to implement your custom logic. Check the below code-snippets for each interface functions

## 1. MyLogger : IEntraEventLogger
Here I prefer to channel incoming logs to Console window

```csharp
//Step 1: Implement interface
public class MyLogger : IEntraEventLogger
{
    public void OnFailure(string message)
    {
        //Step 3: Channel failure logs
        Console.WriteLine(message);
    }

    public void OnInfo(string message)
    {
        //Step 4: Channel information logs
        Console.WriteLine(message);
    }

    public void OnSuccess(string message)
    {
        //Step 5: Channel success logs
        Console.WriteLine(message);
    }
}
```

## 2. MyConfigResolver : IEntraConfigurationResolver
My custom way to read configuration. Here I prefer to read directly from appsettings.json & return as an 'EntraConfig' instance)

```csharp
//Step 1: Implement interface
public class MyConfigResolver : IEntraConfigurationResolver
{
    private readonly IConfiguration configuration;

    public MyConfigResolver(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public EntraConfig Resolve()
    {
        //Step 2: Read configuration from anywhere you prefer
        var config = configuration.GetSection("EntraConfig").Get<EntraConfig>();

        //Step 3: Return as an 'EntraConfig' instance
        return config;
    }
}
```

### 3. MyAuthorizationResolver : IEntraAuthorizationResolver
This is my custom logic to decide who to allow and who to block

```csharp
//Step 1: Implement interface
public class MyAuthorizationResolver : IEntraAuthorizationResolver
{

    //Step 2: Implement your custom authorization logic
    public EntraAuthorizationResult ValidatePolicyAuthorization(HttpContext context, AuthorizationPolicy policy, JwtSecurityToken token)
    {
        //You'll get HttpContext, an active running policy (see appsettings.json to know a policy class's structure). And also, a pre-parsed JWT token from which you can extract and explore claims during your custom authorization procedure.
        //Inject the rest of your required service and build up your logic.


        //Here's my example logic...
        //As you see in appsettings.json below, For active policy I need to check how many scopes are required. Then I compare with scopes available in token. If all required scopes are not present I return false. You can design your own by looking at scopes, roles, or any other claim in your token as well as querying your DB or calling an API.. 

        //Get all scopes from token
        var tokenScopes = token.Claims.Where(x => x.Type == "scp").Select(x => x.Value);
        //Get all scopes required
        var policyScopes = policy.Claims.FirstOrDefault(x => x.Type == "scp")?.Values;
        //Simply check if all required scopes are met
        var isScopesMet = policyScopes.Intersect(tokenScopes).Count() == policyScopes.Count();


        //Step 3: Return an 'EntraAuthorizationResult' that can be called like
        return new EntraAuthorizationResult(isScopesMet, $"Sorry you don't have the following permissions: {string.Join(", ", policyScopes.Except(tokenScopes))} for endpoint: {context.Request.GetDisplayUrl()}");
    }
}
```

### Different ways you can return an `EntraAuthorizationResult`
> `return new EntraAuthorizationResult(true);`  - Indicating you allow the request (Proceeds to endpoint...)

> `return new EntraAuthorizationResult(false);`  - Indicating you blocked the request (403 FORBIDDEN + Global message as API response)

> `return new EntraAuthorizationResult(false, 'My custom message');`  - Indicating you blocked the request (403 FORBIDDEN + Overrided custom new message as API response)

## Configuration
Below is the full configuration in the format of `EntraConfig`

```json
 "EntraConfig": {
   "AppName": "Sample API",
   "ClientId": "xxxxxxxxxxxxxxxxxxxx",

   "EntraEndpoint": {
     "Instance": "https://contoso.b2clogin.com",
     "Domain": "contoso.onmicrosoft.com",
     "TenantId": "xxxxxxxxxxxxxxxx",
     "Policy": "B2C_1_signupsignin",
     "Version": "v2.0"
   },

   "TokenGeneration": {
     "ClientSecret": "xxxxxxxxxxxxxxxx",
     "AppRegistrations": [
       {
         "Name": "CustomerAPI",
         "Scopes": [ "scope1", "scope2" ]
       },
       {
         "Name": "SubscriptionsAPI",
         "Scopes": [ "scope1", "scope2" ]
       }
     ]
   },

   "TokenValidation": {
     "Enable": true,
     "AuthorizationPolicies": [
       {
         "Enable": true,
         "Name": "PolicyA",
         "Claims": [
           {
             "Type": "scp",
             "Values": [ "Files.Read", "Files.Write" ]
           }
         ]
       },
       {
         "Enable": false,
         "Name": "PolicyB",
         "Claims": [
           {
             "Type": "scp",
             "Values": [ "Files.Read", "Files.Write" ]
           }
         ]
       }
     ]
   }
 }
```

EntraConfig.TokenGeneration:

> Not yet implemented. Will be available in future releases

## Configuration Explanations

EntraConfig:

Option |   Expected Value | Example
|--| -----------------| ---
AppName | Name for your API. Used for non-critical purposes like logging | "Sample API"
Client Id| ClientID of your API. Get it from EntraID AppRegistration page | "42d96116-25b5-1a1e-9a8e-ch6a1fd9632f"
EntraEndpoint |     <table>  <tbody>  <tr>  <td>Instance</td> <td>Your AzureAD B2C Instance</td> <td>"https://contoso.b2clogin.com"</td>  </tr>  <tr>  <td>Domain</td>   <td>Your AzureAD B2C Domain</td><td>"contoso.onmicrosoft.com"</td>  </tr>  <tr>  <td>TenantID</td> <td>tenantID. You'll get it from App Registration page</td> <td>"42d96116-25b5-1a1e-9a8e-ch6a1fd9632f"</td>  </tr>  <tr>  <td>Policy</td> <td>Your UserFlow name in B2C</td> <td>"B2C_1_signupsignin"</td>  </tr> <tr>  <td>Version</td> <td>API version. Keep default v2.0</td> <td>"v2.0"</td>  </tr> </tbody>  </table>              |

EntraConfig.TokenValidation:

Option |   Expected Value
|--| -----------------
Enable| Enables or disables Authentication + Authorization globally in your API. Use it like a toggle to enable or disable security
AuthorizationPolicies |  <table> <tbody> <tr> <td>Enable</td><td>Enables or disables a particular policy</td><td>"true"</td></tr><tr> <td>Name</td><td>Name of your policy</td><td>"OnlyUsersWithScopeReadAccess"</td></tr><tr> <td>Claims</td><td><table> <tbody> <tr> <td>Type</td><td>Name of claim this policy is interested to look in. Eg: 'scope'</td><td>"scp"</td></tr><tr> <td>Values</td><td>Expected value to satisfy the policy</td><td>"File.Read"</td></tr></tbody> </table><td>Any number of policies you prefer</td></tr></table>

## Register In DI
Add the above 3 interface implementations also to DI options

```csharp
builder.Services.AddSingleton<MyConfigResolver>();
builder.Services.AddSingleton<MyLogger>();
builder.Services.AddSingleton<MyAuthorizationResolver>();
var serviceProvider = builder.Services.BuildServiceProvider();

builder.Services.AddEntraID(opt =>
{
    opt.EnableEventLogging = true;
    opt.GlobalAuthenticationFailureResponse = "You cannot consume the service.";
    opt.GlobalAuthorizationFailureResponse = "You dont' have enough privilages to access the requested endpoint.";

    //Register all 3
    opt.ConfigurationResolver = serviceProvider.GetService<MyConfigResolver>(); 
    opt.AuthorizationResolver = serviceProvider.GetService<MyAuthorizationResolver>(); 
    opt.SecurityEventLogger = serviceProvider.GetService<MyLogger>();
});
```

> The current version only supports token validation. OBO support and token generation support is not implemented. Validation flow is complete
