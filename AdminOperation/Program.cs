// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using AdminOperation;

if (args.Length != 1)
    return 1;

StreamReader sr = null;
try
{
    sr = new StreamReader(args[0]);
    string line;
    do
    {
        line = sr.ReadLine();
        if (line[0] != '\u001a')
        {
            string[] param = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            IOperation operation = OperationFactory.Create(param[0]);
            operation.Execute(param);
        }
    } while (line[0] != '\u001a');
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    Console.WriteLine("hit any key");
    Console.ReadKey();
    return 1;
}
finally
{
    if (sr != null)
        sr.Close();
}

return 0;
