import { NgModule }             from '@angular/core';
import { BrowserModule  }       from '@angular/platform-browser';

import { SharedModule }         from '../shared/shared.module';
import { CatalogComponent }     from './catalog.component';
import { CatalogService }       from './catalog.service';
import { Pager }                from '../shared/components/pager/pager';

@NgModule({
    imports: [BrowserModule, SharedModule],
    declarations: [CatalogComponent],
    providers: [CatalogService]
})
export class CatalogModule { }
