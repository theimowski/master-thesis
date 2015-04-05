module MusicStore.Path

type IntPath = PrintfFormat<(int -> string),unit,string,string,int>

let home = "/"

module Account =
    let logon = "/account/logon"
    let logoff = "/account/logoff"
    let register = "/account/register"

module Store =
    let overview = "/store"
    let browse = "/store/browse"
    let details : IntPath = "/store/details/%d"

module Cart =
    let overview = "/cart"
    let addAlbum : IntPath = "/cart/add/%d"
    let removeAlbum : IntPath = "/cart/remove/%d"
    let checkout = "/cart/checkout"

module Admin =
    let manage = "/store/manage"
    let createAlbum = "/store/manage/create"
    let editAlbum : IntPath = "/store/manage/edit/%d"
    let deleteAlbum : IntPath = "/store/manage/delete/%d"