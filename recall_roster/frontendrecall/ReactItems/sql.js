const sql = require('mssql');

// Configuration for your SQL Server Express instance
const config = { 
    user: 'sql',
    password: 'password',
  server: 'KOFI-LAPTOP\\SQLEXPRESS',
  database: 'Roster',
  options: {
    encrypt: false, // For non-SSL connections
    trustedConnection:true,
    trustServerCertificate: true, // For self-signed certificates (use false in production)
  },
};

// Function to connect to SQL Server and execute a query
async function runQuery() {
  try {
    // Connect to the database
    await sql.connect(config);

    // Execute a simple query
    const result = await sql.query`SELECT * FROM Contacts`;

    // Log the result
    console.dir(result);
  } catch (err) {
    console.error('Error:', err.message);
  } finally {
    // Close the connection
    await sql.close();
  }
}

// Call the function
runQuery();