const FormerRecalls = () => {


return (
    <div>
        <ToolBar></ToolBar>
        <ul style={{ color: 'white !important' }}>
            {contacts.map(contact => (
              
              <li  key={contact.contact_id} style={{ color: 'white !important' }}>
                <span>
                <h2 className = "list">{contact.firstName + " " + contact.lastName}</h2>
              <Link to={`/editContact/${contact.contact_id}`}>
                <button>Edit</button>
                </Link>
                <button  onClick = {() => {
                  console.log(typeof contact.contact_id);
    
                  handleRemove(contact.contact_id)
                }}>  Remove</button>
              </span>
              </li>
            ))}
          </ul>
          </div>
    )
            
    }
    export default FormerRecalls;