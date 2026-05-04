# 💻 Processador de Reembolsos

## 📌 Descrição

Aplicação console em .NET que processa solicitações de reembolso a partir de um CSV via **stdin**, aplicando regras de negócio e exibindo o resultado no console.

---

## 🧠 Regras de Negócio

* Percentual por tipo:

  * Consulta Médica: 80%
  * Exame de Imagem: 90%
  * Exame Laboratorial: 70%
  * Outros: 50%
* Limite máximo por reembolso: **R$ 500**
* Rejeição para pedidos com mais de **90 dias**
* Fraude:

  * Mais de **5 pedidos em 30 dias**
  * Total de reembolsos acima de **R$ 1500 em 30 dias**

---

## ▶️ Como executar

Já existe um arquivo `entrada.txt` com dados de exemplo.

No terminal do **PowerShell (Visual Studio)**, execute:

```powershell
Get-Content entrada.txt | dotnet run
```

---

## 🧪 Testes

```bash
dotnet test
```

---

## 🏗️ Estrutura

* **Domain** → Entidades
* **Application** → Regras de negócio
* **Infrastructure** → Leitura/escrita CSV
* **Console** → Execução

---

## ⚙️ Tecnologias

* .NET / C#
* Injeção de Dependência
* Logging
* xUnit

---

Print do resultado com Logs e Tabela
<img width="846" height="746" alt="image" src="https://github.com/user-attachments/assets/2ca23d30-1bcb-4867-ba11-f46e67d3c6f0" />
