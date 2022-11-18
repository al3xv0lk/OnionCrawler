# OnionCrawler v1.0
<img
  src="https://user-images.githubusercontent.com/59628368/202736877-1513e4e2-a6d6-4053-9aa9-68c775089709.png"
  alt="Alt text"
  title="Optional title"
  style="display: inline-block; margin: 0 auto; width: 350px">
<img
  src="https://user-images.githubusercontent.com/59628368/202742829-1f45955b-db28-4f43-82bb-dc66dbbeb715.png"
  alt="Alt text"
  title="Optional title"
  style="display: inline-block; margin: 0 auto; width: 350px">
  

## Sobre o projeto
Um crawler assíncrono e com fácil integração a databases OpenSearch(Elasticsearch) para a rede Tor(Deep Web).
## Como funciona
O OnionCrawler utiliza o Tor como proxy, analisa um link inicial e segue buscando todos os links encontrados durante a execução, todos os links obtidos são testados de forma paralela e assíncrona. Somente links válidos e online são retornados.
## Guia de uso
1. Baixe e instale o Tor Browser no site oficial: https://www.torproject.org/download/
2. Baixe o executável do OnionFinder dentro da pasta "Releases"(ou compile com o source)
3. Copie o executável para a pasta raiz do Tor
4. Adicione a linha "SocksPort 127.0.0.1:9050" ao arquivo "torrc", localizado em: "TorBrowser\Data\Tor"
5. Inicie o Tor e conecte.
6. LINUX: Abra o terminal na pasta do executável e `./OnionFinder` para iniciar o programa. WINDOWS: Abra o OnionCrawler normalmente.
7. Digite ou cole o link inicial. Links deverão ser no padrão atual: http://tordexu73joywapk2txdr54jed4imqledpcvcuf75qsas2gwdgksvnyd.onion

## Integração OpenSearch (Em construção)
Caso queira salvar todos os resultados em uma database para futuras pesquisas, a integração é simples:
1. Edite o arquivo "login" na pasta raiz do executável, editando o usuário, senha e link da API da sua DB 
## Atenção
1. O Tor-Browser é necessário para manter o proxy ligado e também facilita as pesquisas.
2. Caso o Tor-Browser ou o OnionCrawler não consigam conectar a rede, reinicie o Tor e reinicie o OnionCrawler.
3. Mesmo otimizado, a velocidade das buscas pode oscilar devido aos nodes do próprio Tor.
## Suporte
Windows e Linux
