import { NgModule }       from '@angular/core';

import { CatalogComponent } from './catalog.component';
import { routing }            from './catalog.routes';


@NgModule({
    imports: [routing],
    declarations: [CatalogComponent]
})
export class CatalogModule { }
