import { Title } from '@angular/platform-browser';
import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';

import { DataService } from './shared/services/data.service';

/*
 * App Component
 * Top Level Component
 */

@Component({
  selector: 'appc-app',
  styleUrls: ['./app.component.scss'],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {


  constructor(private titleService: Title) {

  }

  ngOnInit() {

  }

  public setTitle(newTitle: string) {
    this.titleService.setTitle('eShopOnContainers');
  }
}
