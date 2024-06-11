import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Move } from '../../models/move';

@Component({
  selector: 'app-tile',
  standalone: true,
  imports: [],
  templateUrl: './tile.component.html',
  styleUrl: './tile.component.scss',
})
export class TileComponent {
  @Input({ required: true }) x!: number;
  @Input({ required: true }) y!: number;
  @Input({ required: true }) value!: string;

  @Output() tileClick = new EventEmitter<Move>();

  onTileClick(): void {
    this.tileClick.emit({
      symbol: this.value,
      position: { x: this.x, y: this.y },
    });
  }
}
