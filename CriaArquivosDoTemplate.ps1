param(
    [Parameter(Mandatory=$True)]
    $pasta
)

Write-Host "Pasta enviada: $pasta"

$arquivosTemplate = Get-ChildItem -Path $pasta -Filter *.Template

foreach ($arquivo in $arquivosTemplate) 
{
    $novoNome = $arquivo.FullName -replace ".Template", ""
    Write-Host "Arquivo encontrado: " $arquivo.FullName
    Write-Host "Novo nome: $novoNome"
    if (!(Test-Path -Path $novoNome)) 
    {
        Copy-Item -Path $arquivo.FullName -Destination $novoNome
        Write-Host "Copiado"
    }
}