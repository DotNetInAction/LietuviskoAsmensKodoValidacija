"# LietuviskoAsmensKodoValidacija" 

AsmensKodas asmKodas = new AsmensKodas("387XXXXXXXXXX");

if (asmKodas.IsValid)
{
     Console.WriteLine(asmKodas.Lytis);
     Console.WriteLine(asmKodas.IsValid);
     Console.WriteLine(asmKodas.GimimoData);
}
