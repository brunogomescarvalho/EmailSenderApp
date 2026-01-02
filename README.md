## EmailSenderApp

### API em ASP.NET Core para envio de e-mails
O **EmailSenderApp** é uma API desenvolvida em ASP.NET Core que fornece um endpoint para envio de e-mails por meio de requisições HTTP.  
É ideal para integração com formulários de contato, sistemas de notificação e micro-serviços.

### Configuração
É necessário configurar a conta de e-mail e as credenciais do remetente por meio de variáveis de ambiente ou arquivo `.env`.

```env
EMAIL_SENDER_ADDRESS="seu@email.com"
EMAIL_SENDER_CREDENTIAL="123456"
```

### Payload
Exemplo de payload esperado pelo endpoint de envio:
````
{
  "to": "destinatario@exemplo.com",
  "from": "remetente@exemplo.com",
  "subject": "Assunto da mensagem",
  "message": "Conteúdo da mensagem a ser enviada",
  "name": "Nome do remetente"
}
````

### Implementação
 * Implementada versão de envio utilizando Google (SMTP/Gmail)
 * Rate limit configurado para 3 requisições a cada 10 minutos
