import { NgModule }             from '@angular/core';

import { CatalogComponent }     from './catalog.component';
import { routing }              from './catalog.routes';
import { CatalogService }       from './catalog.service';


@NgModule({
    imports: [routing],
    declarations: [CatalogComponent], 
    providers: [CatalogService]
})
export class CatalogModule { }
