import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Move } from '../../models/move';
import { Position } from '../../models/position';
import { BoardService } from '../../services/board.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

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

  @Output() tileClick = new EventEmitter<Position>();

  constructor() {}

  onTileClick(): void {
    this.tileClick.emit({ 
      x: this.x, 
      y: this.y 
    });
  }
}
