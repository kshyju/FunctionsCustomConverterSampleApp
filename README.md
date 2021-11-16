Sample app demonstrating usage of custom converters.

### What are Input converters ?

Input converters provides a mechanism for populating input parameters of a function from the incoming function invocation. They convert the raw data from the invocation request to types you can work with. There are 7 built-in input converters ships with every functions app which should handle most of the use cases.

### Extending the Input converters

The new converter API allows developers to extend the existing converters by creating custom converters and plugging that into the function invocation pipeline.

### How to create a custom converter?

 * Implement `IInputConverter` interface.

 * Implement the `ConvertAsync` method in your implementation. This method should return an instance of `ValueTask<ConversionResult>`. You may use the below helper methods to create these instances as needed.

 1. `ConversionResult.Unhandled()` - Creates an instance of `ConversionResult` to represent an unhandled input conversion. Your converter's `ConvertAsync` method may be called for every single parameter of the function, if you  register the custom converter globally (via the worker options). So if this method is called for a parameter which is not the type you are interested in populating yourself, you should return an unhandled result. This signals the conversion framework to move on to the next converter.

 2. `ConversionResult Success(object? value)` - Creates an instance of `ConversionResult` to represent a successful input conversion. The `value` parameter could be any object produced from the successful conversion.

 3. `ConversionResult.Failed(Exception exception)` - Creates an instance of  `ConversionResult` to represent a failed input conversion. The exception instance associated with the failure should be passed in to this method.


Here is an example of a converter which populates an instance of `Customer` type if the http request has a customerId query parameter in the request. The sample below is using the customerId to populate the `Name` and `Id` properties of the `Customer` type. But you can write your custom code to populate it anyway you want (Ex: making an HTTP Call to to a REST API to get the Customer data using the customerId and populating the object)

````
public class CustomerConverter : IInputConverter
{
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        if (context.TargetType != typeof(Customer))
        {
            return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
        }

        if (context.Source is string customerIdStr && int.TryParse(customerIdStr, out var customerId))
        {
            var result = new Customer(customerId, "Customer-" + customerId);

            // Successful conversion.
            return new ValueTask<ConversionResult>(ConversionResult.Success(result));
        }

        return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
    }
}

public record Customer(int Id, string Name);
````

The custom converter can be hooked up to the function app in any of the following 3 approaches.

### 1) App level

Add the new converter to `WorkerOptions.InputConverters` while bootstrapping the app. 

```
var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults((workerOptions) =>
            {
                // Add CustomerConverter to the pipeline
                workeroptions.InputConverters.Register<CustomerConverter>();
            })
            .Build();

await host.RunAsync();
```
This option also allows the customers to insert this converter in a specific position or clear all existing built-in converters etc.

##### Adding at a specific index:

The `RegisterAt` method can be used to register the converter at a specific index. The conversion framework calls the `ConvertAsync` method of each converters in the order they were registered.
````
// Adds CustomerConverter to the set of available (built-in) converters, but at index 1. 
workeroptions.InputConverters.RegisterAt<CustomerConverter>(1);
````
##### Clearing existing registered(7 built-in) converters and adding custom converters
````
workeroptions.InputConverters.Clear();
workeroptions.InputConverters.Register<CustomerConverter>();
workeroptions.InputConverters.Register<MyOtherCustomConverter>();
````


### 2) Type level
Decorate the type used as the parameter of the function with `InputConverter` attribute where you will define the custom converter type to be used.

```
[InputConverter(typeof(MyProductConverter))]
public sealed class Product
{
    public int Id { set; get; }
    public string Name { set; get; }
}
```
and

```
[Function("products")]
public HttpResponseData Run(
                 [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req,
                 Product product)
{
   ....    
}
```

### 3) Function parameter level
Decorate the function parameter using the `InputConverter` attribute. The below example is using `EnhancedBlobConverter` converter to populate the `CloudBlob` instance in a blob trigger function.

```
[Function("EnhancedBlobListener")]
public void Run(
    [BlobTrigger("samples-workitems/{name}")] string myBlob, string name, 
    [InputConverter(typeof(EnhancedBlobConverter))]CloudBlob cloudBlob)
{
   ...
}
```

#### Dependency injection in converters

You can inject registered dependencies using constructor injection.

````
public class CustomerConverter : IInputConverter
{
    private readonly ILogger<CustomerConverter> _logger;

    public CustomerConverter(ILogger<CustomerConverter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        // use _logger as needed.
        ....
    }
}

````


To run this sample app, set an environment variable called `AZURE_STORAGE_CONNECTION_STRING` and store the azure storage connection string value in that. The sample app has a converter which enhances a blob trigger by populating a `CloudBlob` object when a new entry is added to the `samples-workitems` container in the storage account.
