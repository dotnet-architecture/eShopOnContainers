import { NgModule, NgModuleFactoryLoader } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule  } from '@angular/platform-browser';
// import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { routing }        from './app.routes';
import { AppService } from './app.service';
import { AppComponent } from './app.component';
import { SharedModule }  from './shared/shared.module';
import { CatalogModule }  from './catalog/catalog.module';
import { OrdersModule }  from './orders/orders.module';
import { BasketModule } from './basket/basket.module';
import { CampaignsModule } from './campaigns/campaigns.module';
import { ToastModule } from 'ng2-toastr/ng2-toastr';


@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserAnimationsModule,
        BrowserModule,
        ToastModule.forRoot(),
        routing,
        HttpModule,
        // Only module that app module loads
        SharedModule.forRoot(),
        CatalogModule,
        OrdersModule,
        BasketModule,
        CampaignsModule        
    ],
    providers: [
        AppService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
