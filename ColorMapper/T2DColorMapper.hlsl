#ifndef T2DCOLORMAPPER_INCLUDED
#define T2DCOLORMAPPER_INCLUDED

bool fall_in_range(float1 val, float1 comp, float1 delta)
{
	return (val >= comp - delta && val <= comp + delta);
}

void map_float(UnityTexture2D    source,
               float1            ssize,
               UnityTexture2D    mapper,
               float1            msize,
               float4            color_in,
               UnitySamplerState ss,
               float1            delta,
               out float4        color_out)
{
	if(ssize == 0.0 || msize == 0.0)
	{
		color_out = color_in;
		return;
	}
	float2 pos_source = {0, 0};
	float2 pos_mapper = {0, 0};

	[unroll(16)]
	for(float1 i = 0.0; i < ssize; i += 1.0)
	{
		pos_source[0] = (i / ssize);
		float4 source_color = SAMPLE_TEXTURE2D(source, ss, pos_source);

		pos_mapper[0] = (i / msize);
		if(msize > ssize)
		{
			pos_mapper[0] = i * (msize / ssize) / msize;
		}
		if(msize < ssize)
		{
			pos_mapper[0] = i * (1 - msize / ssize) / msize;
		}
		float4 mapped_color = SAMPLE_TEXTURE2D(mapper, ss, pos_mapper);

		if(fall_in_range(source_color.r, color_in.r, delta) &&
			fall_in_range(source_color.g, color_in.g, delta) &&
			fall_in_range(source_color.b, color_in.b, delta))
		{
			color_out = mapped_color;
			return;
		}
	}
	color_out = color_in;
}
#endif
