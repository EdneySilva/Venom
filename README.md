Venom Framework
=======================================================================
Venom is a microframework based on .NET Framework 4.5 and ASP.NET MVC 4 
and designed to improve and acelerate the systems development .

Independently your business, almost all systems have core features with
the same technical features, where we will need to control the workflow 
based on the rules from the system, and a consistent security system to
manage the permissions to this features.

When I say same core features, I am talking about CRUD features, which
have the same work:

- Create
- Edit
- Details
- List

And around that we need control the security.

To avoid all of it Venom take on this responsibility to you, through the
core objects of the framework.
When you are using Venom, you automatically leave aside concerns about 
CRUD operations, once it implements the methods:

- Save
- Delete
- ToList
- ToPagedList
- FindItensAsMe
- CreateQuery
- GetById

So you are responsable just to map the context and entities, and if you
choose to use the inheritance from the Venom.Lib.Data.AppDbContext, the
framework create to you the default structure to manage the security to
you.

The ExpressionBuilder
------------------------------------------------------------------------

One of the most amazing features in the framework, it is responsible to 
generate the Linq based on the values of the properties on the objects,
that is, you just populate the properties and say to framework to look
for objects like the model.

VenomObject&lt;T&gt;
------------------------------------------------------------------------

All the business objects need to inherit from this class to have access
the basic system functionalities

You can use it for corporate, business and agency webpages as well as 
