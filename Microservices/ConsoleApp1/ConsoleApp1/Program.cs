// See https://aka.ms/new-console-template for more information

var task1 = PrintAsync("H");
var task2 = PrintAsync("Hi");
var allTasks = Task.WhenAll(task1, task2);
try
{
    await allTasks;
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
    Console.WriteLine($"IsFaulted: {allTasks.IsFaulted}");
    if(allTasks.Exception is not null)
    {
        foreach (var exception in allTasks.Exception.InnerExceptions)
        {
            Console.WriteLine($"InnerException: {exception.Message}");
        }
    }
}
 
async Task PrintAsync(string message)
{
    // если длина строки меньше 3 символов, генерируем исключение
    if (message.Length < 3)
        throw new ArgumentException($"Invalid string: {message}");
    await Task.Delay(1000);     // имитация продолжительной операции
    Console.WriteLine(message);
}