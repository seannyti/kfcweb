# Email Verification Setup Guide

Email verification has been implemented to prevent spam registrations and ensure users have valid email addresses.

## How It Works

1. **User Registration**
   - User fills out registration form
   - Account is created with `EmailVerified = false`
   - Verification email sent automatically (if emails enabled)
   - User redirected to "Check Your Email" page

2. **Email Verification**
   - User receives email with verification link (valid for 24 hours)
   - Clicks link → Redirected to `/verify-email?token=xxx`
   - Token validated → Account activated (`EmailVerified = true`)
   - User can now log in

3. **Login Protection**
   - Unverified users cannot log in
   - Error message: "Please verify your email address before logging in"

4. **Resend Verification**
   - Users can request new verification email
   - Available on both "Check Your Email" page and failed verification page
   - 60-second cooldown to prevent spam

## Email Configuration

### For Production (with real emails):

Add these environment variables to your `.env` file:

```bash
# Enable email sending
EMAIL_ENABLED=true

# SMTP Configuration (example: Gmail)
EMAIL_SMTP_HOST=smtp.gmail.com
EMAIL_SMTP_PORT=587
EMAIL_SMTP_USERNAME=your-email@gmail.com
EMAIL_SMTP_PASSWORD=your-app-password  # NOT your regular password!
EMAIL_FROM_EMAIL=noreply@kfconstruction.com
EMAIL_FROM_NAME=KF Construction
EMAIL_USE_SSL=true

# Frontend URL (for verification links)
FRONTEND_URL=http://23.239.26.52
```

### Using Gmail:

1. Enable 2-Factor Authentication on your Google account
2. Go to: https://myaccount.google.com/apppasswords
3. Generate an "App Password" for "Mail"
4. Use this app password as `EMAIL_SMTP_PASSWORD`

### Using Other SMTP Providers:

**SendGrid:**
```bash
EMAIL_SMTP_HOST=smtp.sendgrid.net
EMAIL_SMTP_PORT=587
EMAIL_SMTP_USERNAME=apikey
EMAIL_SMTP_PASSWORD=your-sendgrid-api-key
```

**AWS SES:**
```bash
EMAIL_SMTP_HOST=email-smtp.us-east-1.amazonaws.com
EMAIL_SMTP_PORT=587
EMAIL_SMTP_USERNAME=your-smtp-username
EMAIL_SMTP_PASSWORD=your-smtp-password
```

**Microsoft 365:**
```bash
EMAIL_SMTP_HOST=smtp.office365.com
EMAIL_SMTP_PORT=587
EMAIL_SMTP_USERNAME=your-email@yourdomain.com
EMAIL_SMTP_PASSWORD=your-password
```

### For Development (without emails):

Set `EMAIL_ENABLED=false` in `.env`:

```bash
EMAIL_ENABLED=false
```

With emails disabled:
- Registration still creates unverified accounts
- Verification emails are logged but not sent
- You'll need to manually verify users in the database

## Database Migration

After deploying, run this SQL to verify all existing users (so they aren't locked out):

```sql
UPDATE Users
SET EmailVerified = 1
WHERE EmailVerified = 0 OR EmailVerified IS NULL;
```

Or use the provided script:
```bash
# From project root
ssh root@23.239.26.52
cd /var/www/kfcweb/MyUsers.Api/Migrations
# Use your database tool to run VerifyExistingUsers.sql
```

## Manual User Verification

If you need to manually verify a user (for testing or support):

```sql
UPDATE Users
SET EmailVerified = 1,
    VerificationToken = NULL,
    VerificationTokenExpiry = NULL
WHERE Email = 'user@example.com';
```

## Testing Email Verification

1. **Without SMTP (Development):**
   - Set `EMAIL_ENABLED=false`
   - Check API logs for verification token
   - Manually construct URL: `http://localhost:5173/verify-email?token=TOKEN_HERE`

2. **With SMTP (Production):**
   - Set `EMAIL_ENABLED=true` and configure SMTP
   - Register new account
   - Check email inbox (and spam folder)
   - Click verification link

## Troubleshooting

### "Failed to send email"
- Check SMTP credentials are correct
- Verify SMTP port (587 for TLS, 465 for SSL)
- Check if your email provider requires app passwords
- Ensure firewall allows outbound SMTP connections

### Users not receiving emails
- Check spam/junk folder
- Verify `EMAIL_FROM_EMAIL` is not blacklisted
- Consider using authenticated domain (SPF/DKIM records)
- Try different SMTP provider

### Token expired
- Tokens are valid for 24 hours
- User can click "Resend Verification Email" to get new token

### Existing users locked out
- Run the `VerifyExistingUsers.sql` migration
- Or manually update specific users with SQL above

## Security Notes

- Verification tokens are single-use (cleared after verification)
- Tokens expire after 24 hours
- Failed verification attempts are logged
- Rate limiting recommended on `/api/auth/resend-verification`
- Use app passwords, never real passwords in configuration
- Consider using environment-specific secrets management (Azure Key Vault, AWS Secrets Manager)

## API Endpoints

- `POST /api/auth/register` - Create account & send verification email
- `GET /api/auth/verify-email?token=xxx` - Verify email with token
- `POST /api/auth/resend-verification` - Request new verification email
  ```json
  {
    "email": "user@example.com"
  }
  ```

## Frontend Routes

- `/register` - Registration form
- `/email-sent` - Post-registration instructions
- `/verify-email?token=xxx` - Token verification page
