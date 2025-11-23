import sys
try:
    import pyodbc
except ImportError:
    print("Installing pyodbc...")
    import subprocess
    subprocess.check_call([sys.executable, "-m", "pip", "install", "pyodbc", "--quiet"])
    import pyodbc

conn_str = "Driver={ODBC Driver 18 for SQL Server};Server=tcp:kfconstruction.database.windows.net,1433;Database=kfconstructiondb-users;Uid=kfconstruction;Pwd=Kat12kat;Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;"

try:
    conn = pyodbc.connect(conn_str)
    cursor = conn.cursor()
    
    # Update all users to verified
    cursor.execute("UPDATE Users SET EmailVerified = 1 WHERE EmailVerified = 0")
    affected = cursor.rowcount
    conn.commit()
    
    # Show results
    cursor.execute("SELECT Name, Email, EmailVerified FROM Users")
    print(f"\nUpdated {affected} user(s) to EmailVerified=true\n")
    print(f"{'Name':<20} {'Email':<30} {'Verified'}")
    print("-" * 60)
    for row in cursor.fetchall():
        print(f"{row[0]:<20} {row[1]:<30} {row[2]}")
    
    cursor.close()
    conn.close()
    print("\n✅ All users are now verified!")
except Exception as e:
    print(f"Error: {e}")
    sys.exit(1)
