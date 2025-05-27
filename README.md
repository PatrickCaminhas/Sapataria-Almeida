# Sistema de Gestão para Sapataria

## Sobre

Este projeto é um sistema administrativo desktop desenvolvido em C# e WinUI 3 (.NET 8) voltado para gerenciamento de vendas e consertos de uma sapataria localizada em Curitiba/PR. O objetivo é fornecer uma interface intuitiva para cadastro, listagem, acompanhamento e análise dos serviços prestados.

## Funcionalidades

* **Módulo de Vendas**

  * Cadastro de vendas
  * Listagem de vendas realizadas

* **Módulo de Consertos**

  * Cadastro de consertos
  * Listagem de consertos em aberto
  * Listagem de consertos finalizados
  * Listagem de consertos entregues

* **Módulo de Informações**

  * Dashboards e gráficos de desempenho das vendas e consertos

## Níveis de Acesso

| Perfil                | Permissões                                  |
| --------------------- | ------------------------------------------- |
| Usuário Comum         | Acesso aos módulos de Vendas e Consertos    |
| Usuário Administrador | Acesso ao módulo de Informações (com senha) |

> A senha de administrador deve ser informada para visualizar relatórios e dashboards.

## Tecnologias e Dependências

* **Linguagem**: C# 10 / .NET 8.0
* **Interface**: WinUI 3 (Windows App SDK 1.7)
* **Banco de Dados**: SQLite (via Entity Framework Core 9.0)
* **MVVM**: CommunityToolkit.Mvvm 8.4.0
* **Gráficos**: LiveChartsCore.SkiaSharpView\.WinUI 2.0.0-rc5.3

## Pré-requisitos

* Windows 10 (build 19041) ou superior
* .NET 8.0 SDK instalado
* Windows App SDK 1.7
* Visual Studio 2022 ou superior com workload de desenvolvimento de aplicativos Windows

## Como Executar

1. Clone este repositório:

   ```bash
   git clone https://github.com/seu-usuario/sapataria-almeida.git
   ```
2. Abra a solução `Sapataria_Almeida.sln` no Visual Studio.
3. Restaure os pacotes NuGet.
4. Compile e execute o projeto.



## Contato

Linkedin https://www.linkedin.com/in/patrickcaminhas/
