namespace TodoApi.Models

open System
open System.Collections.Generic
open System.Globalization
open System.IO
open System.Linq
open Microsoft.Extensions.Localization
open Newtonsoft.Json

type JsonStringLocalizer () =

    let mutable _localizers : List<Localizer> = new List<Localizer>()

    do
        let serializer = new JsonSerializer()
        _localizers <- JsonConvert.DeserializeObject<List<Localizer>>(File.ReadAllText(@"localization.json"))

    interface IStringLocalizer with
        member this.Item
            with get(name: string) =
                let value = this.GetString(name)
                new LocalizedString(name, value, resourceNotFound = false)

        member this.Item
            with get(name: string, [<ParamArray>] arguments: Object[]) =
                let format = this.GetString(name)
                let value = String.Format(format, arguments)
                new LocalizedString(name, value, resourceNotFound = false)
        
        member this.GetAllStrings (includeParentCultures: bool) =
            let selecteds = _localizers.Where(fun l -> l.LocalizedValue.Keys.Any(fun lv -> lv = CultureInfo.CurrentCulture.Name)).Select(fun l -> new LocalizedString(l.Key, snd(l.LocalizedValue.TryGetValue(CultureInfo.CurrentCulture.Name)), true))
            selecteds

        member this.WithCulture (culture: CultureInfo) =
            new JsonStringLocalizer() :> IStringLocalizer

    member this.GetString (name: string) =
        let query = _localizers.Where(fun l -> l.LocalizedValue.Keys.Any(fun lv -> lv = CultureInfo.CurrentCulture.Name))
        let pair = query.FirstOrDefault(fun l -> l.Key = name)
        let selected = pair.LocalizedValue.TryGetValue(CultureInfo.CurrentCulture.Name)
        let value = snd selected
        value