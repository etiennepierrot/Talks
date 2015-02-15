﻿namespace Api

module Controllers =

    open System.Web.Http
    open Representations
    open Common
    open Domain

    type RegisterController
        (
            startProcess : RegisterRepresentation -> Result<Account, Error>
        ) =
        inherit ApiController()
    
        [<HttpPost>]
        [<Route("api/register")>]
        member x.Register(representation : RegisterRepresentation) =

            startProcess representation
                    
//            match startProcess representation with
//                | Success s -> x.Ok() :> IHttpActionResult
//                | Failure f -> x.BadRequest() :> IHttpActionResult

//            match x.ModelState.IsValid with
//                | true -> x.Ok() :> IHttpActionResult
//                | false -> x.BadRequest() :> IHttpActionResult