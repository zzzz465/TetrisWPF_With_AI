local THINK_STATE_COMBO_BUILD = 0 ;
local THINK_STATE_BUSY = 1 ;
local curThinkState_ = THINK_STATE_COMBO_BUILD ;
local COMBO_BUILD_MAX_HEIGHT = 12 ;
local WANT_EMPTY = 3 ;
local COMBO_BUILD_MIN_EVAL = -5000 ;

function Start()
	SetThinkDepth(0) ;
-- 	SetHoldMinDiff(100) ;
-- 	SetThinkDelayFrame(10) ;
-- 	SetMoveDelayFrame(10) ;
-- 	
	SetHoldMinDiff(0) ;
	SetThinkDelayFrame(0) ;
	SetMoveDelayFrame(0) ;
end

function End()
end

function PrepareThink()
	local height = GetBlockHighest() - (GetHeight() - GetBlockLastHeight()) ;
	
	curThinkState_ = THINK_STATE_COMBO_BUILD ;
	
	local curEval = EvalField() ;
	
	if(height >= COMBO_BUILD_MAX_HEIGHT or curEval < COMBO_BUILD_MIN_EVAL) then --�� ������ 5000���ϸ� ������ ����
		curThinkState_ = THINK_STATE_BUSY ;
	end
	
	
 	--Debug(string.format('PrepareThink : state(%d), height(%d), curEval(%d)', curThinkState_, height, curEval)) ;
	--curThinkState_ = THINK_STATE_BUSY ;
end

function EvalField(depth)

	--int x,y,i,full,empty,btype,well,blocked,lblock,rblock;
	-- Intermediate Results
	local HorizontalTransitions = 0;--���η� 0 1 0 1 �ݺ�����
	local FilledLines = 0;
	local HighestBlock = -1;
	local VerticalTransitions = 0;--���� 0 1 0 1 �ݺ�����
	local BlockedCells = 0;--������ ���� ��������
	local Wells = 0; --���ʿ� ���� ������ ���� ���� �������� 
	local TetrisShape = 0 ;
	local StartScore = 0 ;
	local BlockedWells = 0 ; --������ ���� ���� ����
	
	local efield = {} ;
	local x,y,i,full,empty,btype,well,blocked,lblock,rblock;
	local width, height, startHeight ;
	
	width = GetWidth() ;
	--height = GetHeight() ;
	height = GetBlockLastHeight() ;
	startHeight = GetHeight()-GetBlockHighest() ;
	
	HighestBlock = height ;

	-- Copy to eField, Calculate HorizontalTransitions, FilledLines and HighestBlock
	y=height-1 ;
	i=y ;
	while(y>=0) do
		btype=1; full=1; empty=1;
		x=width-1 ;
		while (x>=0) do
			if(0 == GetField(x, y)) then
				efield[x+i*width]=0;
			else
				efield[x+i*width]=1 ;
				empty = 0 ;
			end

			if(btype ~= efield[x+i*width]) then
				btype = efield[x+i*width];
				full=0;
				HorizontalTransitions = HorizontalTransitions + 1;
			end
			x = x-1 ;
		end
		
		if(0 == full) then
			i = i-1;
		else
			FilledLines = FilledLines + 1 ;
		end
		
		if(0 == btype) then
			HorizontalTransitions = HorizontalTransitions + 1;
		end
		
		if(i < startHeight and empty == 1) then
			HighestBlock = height-(i+2);
			break ;
		end
		
		y = y-1 ;
	end
	
	
	x=0 ;
	while(x<width) do
		btype=0; blocked=0; blockedHeight = 0 ;
		y = height-HighestBlock ;
		while(y<height) do
			
			if(btype~=efield[x+y*width]) then
				if(blocked == 0) then
					blocked=1;
					blockedHeight = y ;
				end
				btype=efield[x+y*width];
				VerticalTransitions= VerticalTransitions+1;
			end
			
			-- ������ ������ ���� �ڸ��� ����ִٸ� BlockedCells����
			if(blocked == 1 and 0 == efield[x+y*width]) then
				BlockedCells = BlockedCells + 1 ;
				BlockedWells = BlockedWells + (y - blockedHeight) ;
			end
			
			if(x==0) then
				lblock=1;
			else
				lblock = efield[(x-1)+y*width];
			end
			
			if(x==width-1) then
				rblock=1;
			else
				rblock = efield[(x+1)+y*width];
			end
			
			if(curThinkState_ == THINK_STATE_COMBO_BUILD and blocked == 0 and lblock == 1) then --�¿쿡 ���� ������
				i=y ;
				while(i<height) do
					local lpos = -1 ;
					local rpos = width ;
					local emptyCnt = 0 ;
					j=0 ;
					while(j < width) do
						if(efield[j+i*width] == 0) then
							if(lpos < 0) then 
								lpos = j ;
							end
							emptyCnt = emptyCnt + 1 ;
						else
							if(lpos >= 0 and rpos >= width) then --���� ���ٰ� �ٽ� ���ܳ� ����
								rpos = j ;
							end
						end
						j = j+1 ;
					end
					
-- 					Debug(string.format('shape : %d(empty:%d, lpos(%d), rpos(%d), x(%d))(%d%d%d%d%d%d%d%d%d%d)',
-- 						 i,
-- 						 emptyCnt,
-- 						 lpos,
-- 						 rpos,
-- 						 x,
-- 						 efield[0+i*width],
-- 						 efield[1+i*width],
-- 						 efield[2+i*width],
-- 						 efield[3+i*width],
-- 						 efield[4+i*width],
-- 						 efield[5+i*width],
-- 						 efield[6+i*width],
-- 						 efield[7+i*width],
-- 						 efield[8+i*width],
-- 						 efield[9+i*width]
-- 						 )) ;
						 
					if(emptyCnt == WANT_EMPTY and rpos - lpos == WANT_EMPTY and lpos == x) then
						TetrisShape = TetrisShape + 1 ;
-- 						Debug(string.format('tetris shape : %d(empty:%d, lpos(%d), rpos(%d))(%d%d%d%d%d%d%d%d%d%d)',
-- 						 i,
-- 						 emptyCnt,
-- 						 lpos,
-- 						 rpos,
-- 						 efield[0+i*width],
-- 						 efield[1+i*width],
-- 						 efield[2+i*width],
-- 						 efield[3+i*width],
-- 						 efield[4+i*width],
-- 						 efield[5+i*width],
-- 						 efield[6+i*width],
-- 						 efield[7+i*width],
-- 						 efield[8+i*width],
-- 						 efield[9+i*width]
-- 						 )) ;
					else
						break ;--���� ������ ����
					end
					i = i + 1 ;
				end
			end
			
			
			
			if(lblock == 1 and rblock == 1) then --�¿쿡 ���� ������
				i=y ;
				while(i<height) do
					if(efield[x+i*width]~=0) then --�߰��� ����ִٸ�
						break ;
					end
					Wells=Wells+1; --well�� ����
					--Debug(string.format('wells line : %d', i)) ;
					i = i+1 ;
				end
			end
			y = y+1 ;
		end
		if(0 == btype) then
			VerticalTransitions=VerticalTransitions+1;
		end
		x=x+1 ;
	end
	
	
	
	
	local FinalScore = 0 ;
	local comboCnt = GetComboCount() ;
	
		
	if(curThinkState_ == THINK_STATE_COMBO_BUILD) then
		if(FilledLines > 0) then
			if(comboCnt >= 5) then --5�޺� �̻��̸� �� ����ġ�� �ش�.
				StartScore = StartScore + 5000 ;
			elseif(comboCnt >= 1) then --1�޺� �̻��̸� ����ġ
				StartScore = StartScore + 2000 ;
			else --�޺��� �ƴϸ� �����ϰ�..
				StartScore = StartScore - 1000 ;
			end
		end
		
	
 		FinalScore = 
			StartScore +
			(-10 * HighestBlock)+
			(-10 * HorizontalTransitions)+
			(-10 * VerticalTransitions)+
			(-1000 * BlockedCells)+
			(-9 * Wells)+
			--( 1 * FilledLines)+
			(100 * TetrisShape)+
			(-10 * BlockedWells) ;
	elseif(curThinkState_ == THINK_STATE_BUSY) then
	
		if(FilledLines > 0) then
			if(comboCnt >= 5) then --5�޺� �̻��̸� �� ����ġ�� �ش�.
				StartScore = StartScore + 5000 ;
			elseif(comboCnt >= 1) then --1�޺� �̻��̸� ����ġ
				StartScore = StartScore + 2000 ;
			end
		end
			
		FinalScore = 
			StartScore +
			(-100 * HighestBlock)
			+(-100 * HorizontalTransitions)
			+(-100 * VerticalTransitions)
			+(-10 * BlockedCells)
			+(-9 * Wells)
			+(100 * FilledLines)
			+(0 * TetrisShape)
			+(-10 * BlockedWells)
			 ;
	
	end
	
	
	
	--if(curThinkState_ == THINK_STATE_COMBO_BUILD) then
	--	if(FilledLines > 0 and FilledLines < 3) then
			--Debug(string.format('Score : %d, FillLine : %d, height : %d, blockCells : %d', FinalScore, FilledLines, HighestBlock, BlockedCells)) ;
	--	end
	--end
	
-- 	Debug(string.format('Score(%d), StartScore(%d), HighestBlock(%d), HorizontalTransitions(%d), VerticalTransitions(%d), BlockedCells(%d), Wells(%d), FilledLine(%d), TetrisShape(%d), BlockedWells(%d)',
-- 		FinalScore,
-- 		StartScore,
-- 		HighestBlock,
-- 		HorizontalTransitions,
-- 		VerticalTransitions,
-- 		BlockedCells,
-- 		Wells,
-- 		FilledLines,
-- 		TetrisShape,
-- 		BlockedWells
-- 		)) ;
		
		
	return FinalScore;
	
end
