import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'appc-catalog',
  styleUrls: ['./catalog.component.scss'],
  templateUrl: './catalog.component.html'
})
export class CatalogComponent implements OnInit {
  constructor() { }

  ngOnInit() {
    console.log('catalog component loaded');
  }

}
