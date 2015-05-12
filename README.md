# Automação de Extratos Para Gerenciadores Financeiros
Programa para automatizar a exportação de extratos bancários e importações em gerenciadores financeiros.

A ideia é fazer um programa que acesse de alguma forma os extratos dos bancos e importe esses dados em alguns gerenciadores financeiros.

### Bancos Implementados
- Santander via Selenium
 - Uma conta corrente
 - Todos os cartões de crédito
- CEF via Selenium utilizando Mozilla Firefox
 - Somente conta corrente 
 
### Gereneciadores Financeiros implementados
- Minhas Economias via Selenium

### Configurações
- É necessário salvar os arquivos banco.template.json e gerenciador.template.json como banco.json e gerenciador.json na mesma pasta que eles se encontram.
- Após isso edite os arquivos seguindo o modelo apresentado.